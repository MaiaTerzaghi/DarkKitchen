import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ProductService } from '../../../backend/services/product/product.service';
import ProductResponse from '../../../backend/services/product/models/ProductResponse';

function imagesValidator(control: AbstractControl): ValidationErrors | null {
  const value: string = control.value ?? '';
  if (!value.trim()) {
    return null; // el 'required' ya cubre el vacío
  }
  const list = value
    .split(',')
    .map((s) => s.trim())
    .filter((s) => s.length > 0);

  if (list.length > 3) {
    return { imagesMax: true };
  }
  if (list.some((img) => !img.toLowerCase().endsWith('.jpg'))) {
    return { imagesJpg: true };
  }
  return null;
}

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
    images: new FormControl('', [Validators.required, imagesValidator]),
  });

  errorMessage: string = '';
  loading: boolean = false;

  get isEditMode(): boolean {
    return this.productToEdit !== null;
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
    }
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
        this.productForm.reset();
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
    this.productForm.reset();
    this.errorMessage = '';
    this.close.emit();
  }
}