import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
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
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  importers: ImporterInfo[] = [];
  loadingImporters: boolean = false;

  selectedImporter: string = '';
  selectedFile: File | null = null;

  importing: boolean = false;
  errorMessage: string = '';
  result: ImportResult | null = null;

  isDragging: boolean = false;

  constructor(private readonly _importService: ImportService) {}

  ngOnInit(): void {
    this.loadImporters();
  }

  private loadImporters(): void {
    this.loadingImporters = true;

    this._importService.getImporters().subscribe({
      next: (data) => {
        this.importers = data;
        this.loadingImporters = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar importadores';
        this.loadingImporters = false;
      },
    });
  }

  public selectImporter(name: string): void {
    this.selectedImporter = name;
    this.result = null;
    this.errorMessage = '';
  }

  public getPluginIcon(name: string): string {
    const lower = name.toLowerCase();
    if (lower.includes('json')) return 'data_object';
    if (lower.includes('xml')) return 'code';
    if (lower.includes('csv')) return 'table_chart';
    return 'extension';
  }

  public getPluginDescription(name: string): string {
    const lower = name.toLowerCase();
    if (lower.includes('json')) return 'Importa productos desde archivos JSON estructurados';
    if (lower.includes('xml')) return 'Importa productos desde archivos XML';
    if (lower.includes('csv')) return 'Importa productos desde archivos CSV delimitados';
    return 'Importador de productos';
  }

  public getPluginFormat(name: string): string {
    const lower = name.toLowerCase();
    if (lower.includes('json')) return 'json';
    if (lower.includes('xml')) return 'xml';
    if (lower.includes('csv')) return 'csv';
    return lower;
  }

  public triggerFileInput(): void {
    this.fileInputRef?.nativeElement?.click();
  }

  public onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.result = null;
      this.errorMessage = '';
    }
  }

  public onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  public onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  public onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.selectedFile = event.dataTransfer.files[0];
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

  public clearResults(): void {
    this.result = null;
    this.selectedFile = null;
    this.selectedImporter = '';
    this.errorMessage = '';
  }
}
