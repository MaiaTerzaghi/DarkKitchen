import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { AuthenticationRoutingModule } from './authentication-routing.module';
import { AuthenticationPageComponent } from './authentication-page/authentication-page.component';
import { LoginFormComponent } from './login-form/login-form.component';

@NgModule({
  declarations: [AuthenticationPageComponent, LoginFormComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    AuthenticationRoutingModule,
  ],
})
export class AuthenticationModule {}
