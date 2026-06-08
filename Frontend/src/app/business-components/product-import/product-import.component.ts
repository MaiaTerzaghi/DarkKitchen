import { Component, OnInit } from '@angular/core';
import { ImportService } from '../../../backend/services/import/import.service';
import ImporterInfo from '../../../backend/services/import/models/ImporterInfo';
import ImportResult from '../../../backend/services/import/models/ImportResult';

@Component({
  selector: 'app-product-import',
  templateUrl: './product-import.component.html',
  standalone: false,
  styleUrls: ['./product-import.component.css'],
})
export class ProductImportComponent implements OnInit {
  importers: ImporterInfo[] = [];
  loadingImporters: boolean = false;

  selectedImporter: string = '';
  selectedFile: File | null = null;

  importing: boolean = false;
  errorMessage: string = '';
  result: ImportResult | null = null;

  constructor(private readonly _importService: ImportService) {}

  ngOnInit(): void {
    this.loadImporters();
  }

  private loadImporters(): void {
    this.loadingImporters = true;

    this._importService.getImporters().subscribe({
      next: (data) => {
        this.importers = data;
        if (data.length > 0) {
          this.selectedImporter = data[0].name;
        }
        this.loadingImporters = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar importadores';
        this.loadingImporters = false;
      },
    });
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.result = null;
      this.errorMessage = '';
    }
  }

  public get canImport(): boolean {
    return !!this.selectedImporter && !!this.selectedFile && !this.importing;
  }

  public importProducts(): void {
    if (!this.canImport || !this.selectedFile) return;

    this.importing = true;
    this.errorMessage = '';
    this.result = null;

    const reader = new FileReader();

    reader.onload = () => {
      const content = reader.result as string;

      this._importService
        .importProducts(this.selectedImporter, content, this.selectedFile!.name)
        .subscribe({
          next: (data) => {
            this.result = data;
            this.importing = false;
          },
          error: (err) => {
            this.errorMessage = err || 'Error al importar productos';
            this.importing = false;
          },
        });
    };

    reader.onerror = () => {
      this.errorMessage = 'Error al leer el archivo';
      this.importing = false;
    };

    reader.readAsText(this.selectedFile);
  }
}
