import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../../backend/services/user/user.service';

@Component({
  selector: 'app-register-form',
  templateUrl: './register-form.component.html',
  standalone: false,
  styleUrls: ['./register-form.component.css'],
})
export class RegisterFormComponent {
  registerForm = new FormGroup({
    name: new FormControl('', [Validators.required]),
    lastName: new FormControl('', [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
    confirmPassword: new FormControl('', [Validators.required]),
  });

  errorMessage: string = '';
  successMessage: string = '';
  showPassword: boolean = false;
  showConfirmPassword: boolean = false;

  constructor(
    private readonly _userService: UserService,
    private readonly _router: Router
  ) {}

  public togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  public toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  public onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    const password = this.registerForm.value.password!;
    const confirmPassword = this.registerForm.value.confirmPassword!;

    if (password !== confirmPassword) {
      this.errorMessage = 'Las contraseñas no coinciden';
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    const data = {
      name: this.registerForm.value.name!,
      lastName: this.registerForm.value.lastName!,
      email: this.registerForm.value.email!,
      phone: this.registerForm.value.phone!,
      password: password,
    };

    this._userService.register(data).subscribe({
      next: () => {
        this.successMessage = 'Cuenta creada exitosamente. Redirigiendo al login...';
        setTimeout(() => {
          this._router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.errorMessage = err || 'Error al crear la cuenta';
      },
    });
  }
}
