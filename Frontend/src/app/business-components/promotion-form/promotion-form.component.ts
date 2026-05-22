import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PromotionService } from '../../../backend/services/promotion/promotion.service';

@Component({
  selector: 'app-promotion-form',
  templateUrl: './promotion-form.component.html',
  standalone: false,
  styleUrls: ['./promotion-form.component.css'],
})
export class PromotionFormComponent {
  @Input() visible: boolean = false;
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

  constructor(private readonly _promotionService: PromotionService) {}

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

    this._promotionService.create(data).subscribe({
      next: () => {
        this.loading = false;
        this.promotionForm.reset();
        this.saved.emit();
      },
      error: (err) => {
        this.errorMessage = err || 'Error al crear la promoción';
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
