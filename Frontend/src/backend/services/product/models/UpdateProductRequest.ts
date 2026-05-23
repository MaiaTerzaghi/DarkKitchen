export default interface UpdateProductRequest {
  code: string;
  name: string;
  description: string;
  price: number;
  commercialLine: string;
  category: string;
  images: string;
  isActive: boolean;
}