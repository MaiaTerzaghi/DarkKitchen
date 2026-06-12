import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../backend/services/user/user.service';
import UserResponse from '../../../backend/services/user/models/UserResponse';
import CreateUserRequest from '../../../backend/services/user/models/CreateUserRequest';
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

  // Paginación
  currentPage: number = 1;
  pageSize: number = 20;
  totalCount: number = 0;

  showModal: boolean = false;
  editingUser: UserResponse | null = null;

  formData = {
    name: '',
    lastName: '',
    email: '',
    phone: '',
    password: '',
    role: 1,
  };

  constructor(private readonly _userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.errorMessage = '';
    this._userService.getUsers(undefined, undefined, this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.users = response.items;
        this.totalCount = response.totalCount;
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
      result = result.filter((u) => u.role === Number(this.filterRole));
    }

    this.filteredUsers = result;
  }

  getRoleLabel(role: number): string {
    const roles: Record<number, string> = {
      0: 'Cliente',
      1: 'Preparador',
      2: 'Administrador',
    };
    return roles[role] || 'Desconocido';
  }

  getRoleClass(role: number): string {
    const classes: Record<number, string> = {
      0: 'role-client',
      1: 'role-dispatcher',
      2: 'role-admin',
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
      role: user.role,
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
      const request: CreateUserRequest = {
        name: this.formData.name,
        lastName: this.formData.lastName,
        email: this.formData.email,
        phone: this.formData.phone,
        password: this.formData.password,
        role: this.formData.role,
      };

      this._userService.createUser(request).subscribe({
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

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadUsers();
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

}
