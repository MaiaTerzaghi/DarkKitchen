import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../backend/services/user/user.service';
import UserResponse from '../../../backend/services/user/models/UserResponse';
import CreateStaffUserRequest from '../../../backend/services/user/models/CreateStaffUserRequest';
import UpdateUserRequest from '../../../backend/services/user/models/UpdateUserRequest';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  standalone: false,
  styleUrls: ['./user-list.component.css'],
})
export class UserListComponent implements OnInit {
  users: UserResponse[] = [];
  filteredUsers: UserResponse[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  searchText: string = '';
  filterRole: string = 'all';

  showModal: boolean = false;
  editingUser: UserResponse | null = null;

  formData = {
    name: '',
    lastName: '',
    email: '',
    phone: '',
    password: '',
    role: 2,
  };

  constructor(private readonly _userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.errorMessage = '';
    this._userService.getUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = err || 'Error al cargar usuarios';
        this.loading = false;
      },
    });
  }

  applyFilters(): void {
    let result = this.users;

    if (this.searchText) {
      const search = this.searchText.toLowerCase();
      result = result.filter(
        (u) =>
          u.name.toLowerCase().includes(search) ||
          u.lastName.toLowerCase().includes(search) ||
          u.email.toLowerCase().includes(search)
      );
    }

    if (this.filterRole !== 'all') {
      result = result.filter((u) => u.role === this.filterRole);
    }

    this.filteredUsers = result;
  }

  getRoleLabel(role: string): string {
    const roles: Record<string, string> = {
      Administrative: 'Administrador',
      Dispatcher: 'Preparador',
      Client: 'Cliente',
    };
    return roles[role] || role;
  }

  getRoleClass(role: string): string {
    const classes: Record<string, string> = {
      Administrative: 'role-admin',
      Dispatcher: 'role-dispatcher',
      Client: 'role-client',
    };
    return classes[role] || '';
  }

  openCreateModal(): void {
    this.editingUser = null;
    this.formData = { name: '', lastName: '', email: '', phone: '', password: '', role: 2 };
    this.showModal = true;
  }

  openEditModal(user: UserResponse): void {
    this.editingUser = user;
    this.formData = {
      name: user.name,
      lastName: user.lastName,
      email: user.email,
      phone: user.phone,
      password: '',
      role: this.roleStringToNumber(user.role),
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingUser = null;
  }

  saveUser(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.editingUser) {
      const request: UpdateUserRequest = {
        name: this.formData.name,
        lastName: this.formData.lastName,
        email: this.formData.email,
        phone: this.formData.phone,
        role: this.formData.role,
      };
      if (this.formData.password) {
        request.password = this.formData.password;
      }

      this._userService.updateUser(this.editingUser.id, request).subscribe({
        next: () => {
          this.successMessage = 'Usuario actualizado correctamente';
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          this.errorMessage = err || 'Error al actualizar usuario';
        },
      });
    } else {
      const request: CreateStaffUserRequest = {
        name: this.formData.name,
        lastName: this.formData.lastName,
        email: this.formData.email,
        phone: this.formData.phone,
        password: this.formData.password,
        role: this.formData.role,
      };

      this._userService.createStaffUser(request).subscribe({
        next: () => {
          this.successMessage = 'Usuario creado correctamente';
          this.closeModal();
          this.loadUsers();
        },
        error: (err) => {
          this.errorMessage = err || 'Error al crear usuario';
        },
      });
    }
  }

  deleteUser(user: UserResponse): void {
    if (!confirm(`¿Estás seguro de eliminar a ${user.name} ${user.lastName}?`)) return;

    this._userService.deleteUser(user.id).subscribe({
      next: () => {
        this.successMessage = 'Usuario eliminado correctamente';
        this.loadUsers();
      },
      error: (err) => {
        this.errorMessage = err || 'Error al eliminar usuario';
      },
    });
  }

  private roleStringToNumber(role: string): number {
    const map: Record<string, number> = {
      Client: 0,
      Administrative: 1,
      Dispatcher: 2,
    };
    return map[role] ?? 2;
  }
}
