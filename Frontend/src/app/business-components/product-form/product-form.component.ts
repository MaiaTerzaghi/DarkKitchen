import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../../backend/services/product/product.service';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  standalone: false,
  styleUrls: ['./product-form.component.css'],
})
export class ProductFormComponent implements OnChanges {
  @Input() visible: boolean = false;
  @Input() productToEdit: ProductResponse | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  private static readonly MaxImages = 3;
  private static readonly MaxImageBytes = 500 * 1024;

  productForm = new FormGroup({
    code: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
      Validators.maxLength(20),
    ]),
    name: new FormControl('', [
      Validators.required,
      Validators.minLength(10),
      Validators.maxLength(50),
    ]),
    description: new FormControl('', [
      Validators.required,
      Validators.minLength(20),
      Validators.maxLength(500),
    ]),
    price: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(0.01),
    ]),
    commercialLine: new FormControl('', [Validators.required]),
    category: new FormControl('', [Validators.required]),
    images: new FormControl('', [Validators.required]),
  });

  previews: string[] = [];
  imageError: string = '';
  errorMessage: string = '';
  loading: boolean = false;
  showConfirmExit: boolean = false;

  get isEditMode(): boolean {
    return this.productToEdit !== null;
  }

  get isDirty(): boolean {
    return this.productForm.dirty;
  }

  constructor(private readonly _productService: ProductService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['productToEdit'] && this.productToEdit) {
      this.productForm.patchValue({
        code: this.productToEdit.code,
        name: this.productToEdit.name,
        description: this.productToEdit.description,
        price: this.productToEdit.price,
        commercialLine: this.productToEdit.commercialLine,
        category: this.productToEdit.category,
        images: this.productToEdit.images,
      });
      this.previews = this.toImageList(this.productToEdit.images);
    }
  }

  public async onImagesSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const files = input.files;
    this.imageError = '';

    if (!files || files.length === 0) {
      return;
    }

    if (this.previews.length + files.length > ProductFormComponent.MaxImages) {
      this.imageError = 'Se permiten hasta 3 imágenes.';
      input.value = '';
      return;
    }

    for (let i = 0; i < files.length; i++) {
      const file = files[i];

      if (file.type !== 'image/jpeg') {
        this.imageError = 'Las imágenes deben ser en formato .jpg.';
        input.value = '';
        return;
      }
      if (file.size > ProductFormComponent.MaxImageBytes) {
        this.imageError = 'Cada imagen debe pesar como máximo 500 KB.';
        input.value = '';
        return;
      }

      this.previews.push(await this.readAsBase64(file));
    }

    this.updateImagesControl();
    input.value = ''; // permite volver a elegir y agregar más
  }

  public removeImage(index: number): void {
    this.previews.splice(index, 1);
    this.updateImagesControl();
  }

  private updateImagesControl(): void {
    this.productForm.patchValue({ images: this.previews.join(',') });
    this.productForm.get('images')?.markAsDirty();
  }

  public onSubmit(): void {
    if (this.productForm.invalid) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const base = {
      code: this.productForm.value.code!,
      name: this.productForm.value.name!,
      description: this.productForm.value.description!,
      price: this.productForm.value.price!,
      commercialLine: this.productForm.value.commercialLine!,
      category: this.productForm.value.category!,
      images: this.productForm.value.images!,
    };

    const request$ = this.isEditMode
      ? this._productService.update(this.productToEdit!.id, {
          ...base,
          isActive: this.productToEdit!.isActive,
        })
      : this._productService.create(base);

    request$.subscribe({
      next: () => {
        this.loading = false;
        this.resetForm();
        this.saved.emit();
      },
      error: (err) => {
        const action = this.isEditMode ? 'actualizar' : 'crear';
        this.errorMessage = err || `Error al ${action} el producto`;
        this.loading = false;
      },
    });
  }

  public onClose(): void {
    if (this.productForm.dirty) {
      this.showConfirmExit = true;
      return;
    }
    this.resetForm();
    this.close.emit();
  }

  public confirmExit(): void {
    this.showConfirmExit = false;
    this.resetForm();
    this.close.emit();
  }

  public cancelExit(): void {
    this.showConfirmExit = false;
  }

  private resetForm(): void {
    this.productForm.reset();
    this.previews = [];
    this.imageError = '';
    this.errorMessage = '';
  }

  private readAsBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => {
        const result = reader.result as string;
        resolve(result.split(',')[1]); 
      };
      reader.onerror = () => reject(reader.error);
      reader.readAsDataURL(file);
    });
  }

  private toImageList(images: string): string[] {
    return images
      ? images.split(',').map((i) => i.trim()).filter((i) => i.length > 0)
      : [];
  }
}