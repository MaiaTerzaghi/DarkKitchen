export default interface CreateProductRequest {
  code: string;
  name: string;
  description: string;
  price: number;
  commercialLine: string;
  category: string;
  images: string;
}