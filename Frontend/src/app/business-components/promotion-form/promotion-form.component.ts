import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PromotionService } from '../../../backend/services/promotion/promotion.service';
import PromotionResponse from '../../../backend/services/promotion/models/PromotionResponse';

@Component({
  selector: 'app-promotion-form',
  templateUrl: './promotion-form.component.html',
  standalone: false,
  styleUrls: ['./promotion-form.component.css'],
})
export class PromotionFormComponent implements OnChanges {
  @Input() visible: boolean = false;
  @Input() promotionToEdit: PromotionResponse | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  promotionForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    discountPercentage: new FormControl<number | null>(null, [
      Validators.required,
      Validators.min(1),
      Validators.max(100),
    ]),
    validFrom: new FormControl('', [Validators.required]),
    validTo: new FormControl('', [Validators.required]),
  });

  errorMessage: string = '';
  loading: boolean = false;

  get isEditMode(): boolean {
    return this.promotionToEdit !== null;
  }

  constructor(private readonly _promotionService: PromotionService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['promotionToEdit'] && this.promotionToEdit) {
      this.promotionForm.patchValue({
        name: this.promotionToEdit.name,
        discountPercentage: this.promotionToEdit.discountPercentage,
        validFrom: this.promotionToEdit.validFrom.substring(0, 10),
        validTo: this.promotionToEdit.validTo.substring(0, 10),
      });
    }
  }

  public onSubmit(): void {
    if (this.promotionForm.invalid) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const data = {
      name: this.promotionForm.value.name!,
      discountPercentage: this.promotionForm.value.discountPercentage!,
      validFrom: this.promotionForm.value.validFrom!,
      validTo: this.promotionForm.value.validTo!,
    };

    const request$ = this.isEditMode
      ? this._promotionService.update(this.promotionToEdit!.id, data)
      : this._promotionService.create(data);

    request$.subscribe({
      next: () => {
        this.loading = false;
        this.promotionForm.reset();
        this.saved.emit();
      },
      error: (err) => {
        const action = this.isEditMode ? 'actualizar' : 'crear';
        this.errorMessage = err || `Error al ${action} la promoción`;
        this.loading = false;
      },
    });
  }

  public onClose(): void {
    this.promotionForm.reset();
    this.errorMessage = '';
    this.close.emit();
  }
}
