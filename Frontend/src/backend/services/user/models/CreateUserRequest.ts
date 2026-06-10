export default interface CreateUserRequest {
  name: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
  role?: number;
}
