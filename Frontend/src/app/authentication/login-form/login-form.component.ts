import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../backend/services/auth/auth.service';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  standalone: false,
  styleUrls: ['./login-form.component.css'],
})
export class LoginFormComponent {
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  errorMessage: string = '';
  showPassword: boolean = false;

  constructor(
    private readonly _authService: AuthService,
    private readonly _router: Router
  ) {}

  public togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  public onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.errorMessage = '';

    const credentials = {
      email: this.loginForm.value.email!,
      password: this.loginForm.value.password!,
    };

    this._authService.login(credentials).subscribe({
      next: () => {
        this._router.navigate(['/home']);
      },
      error: (err) => {
        this.errorMessage = 'Credenciales inválidas';
      },
    });
  }
}
