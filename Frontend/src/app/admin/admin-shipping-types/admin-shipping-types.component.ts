import { Component, OnInit } from '@angular/core';
import { ShippingTypeService } from '../../../backend/services/shipping-type/shipping-type.service';
import ShippingTypeResponse from '../../../backend/services/shipping-type/models/ShippingTypeResponse';

@Component({
  selector: 'app-admin-shipping-types',
  templateUrl: './admin-shipping-types.component.html',
  standalone: false,
  styleUrls: ['./admin-shipping-types.component.css'],
})
export class AdminShippingTypesComponent implements OnInit {
  shippingTypes: ShippingTypeResponse[] = [];
  loading: boolean = false;
  errorMessage: string = '';

  showModal: boolean = false;
  editingType: ShippingTypeResponse | null = null;
  saving: boolean = false;

  formName: string = '';
  formCost: number = 0;

  constructor(private readonly _shippingTypeService: ShippingTypeService) {}

  ngOnInit(): void {
    this.loadShippingTypes();
  }

  public loadShippingTypes(): void {
    this.loading = true;
    this.errorMessage = '';

    this._shippingTypeService.getAll().subscribe({
      next: (data) => {
        this.shippingTypes = data;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar tipos de envío';
        this.loading = false;
      },
    });
  }

  public openCreate(): void {
    this.editingType = null;
    this.formName = '';
    this.formCost = 0;
    this.errorMessage = '';
    this.showModal = true;
  }

  public openEdit(type: ShippingTypeResponse): void {
    this.editingType = type;
    this.formName = type.name;
    this.formCost = type.cost;
    this.errorMessage = '';
    this.showModal = true;
  }

  public closeModal(): void {
    this.showModal = false;
    this.editingType = null;
    this.errorMessage = '';
  }

  public save(): void {
    if (!this.formName.trim()) {
      this.errorMessage = 'El nombre es obligatorio';
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    const data = { name: this.formName.trim(), cost: this.formCost };

    if (this.editingType) {
      this._shippingTypeService.update(this.editingType.id, data).subscribe({
        next: (updated) => {
          const idx = this.shippingTypes.findIndex((t) => t.id === updated.id);
          if (idx !== -1) {
            this.shippingTypes[idx] = updated;
          }
          this.saving = false;
          this.closeModal();
        },
        error: (err) => {
          this.errorMessage = err || 'Error al actualizar el tipo de envío';
          this.saving = false;
        },
      });
    } else {
      this._shippingTypeService.create(data).subscribe({
        next: (created) => {
          this.shippingTypes.push(created);
          this.saving = false;
          this.closeModal();
        },
        error: (err) => {
          this.errorMessage = err || 'Error al crear el tipo de envío';
          this.saving = false;
        },
      });
    }
  }

  public formatCurrency(value: number): string {
    return '$' + value.toLocaleString('es-UY', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
  }
}
