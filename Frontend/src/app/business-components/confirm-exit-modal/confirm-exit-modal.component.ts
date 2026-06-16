import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-exit-modal',
  templateUrl: './confirm-exit-modal.component.html',
  styleUrls: ['./confirm-exit-modal.component.css'],
  standalone: false,
})
export class ConfirmExitModalComponent {
  @Input() visible: boolean = false;
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onConfirm(): void {
    this.confirm.emit();
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
