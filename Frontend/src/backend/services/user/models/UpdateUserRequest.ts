export default interface UpdateUserRequest {
  name: string;
  lastName: string;
  email: string;
  phone: string;
  password?: string;
  role: number;
}
