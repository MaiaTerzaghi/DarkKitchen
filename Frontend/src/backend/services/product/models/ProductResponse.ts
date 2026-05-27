export default interface ProductResponse {
  id: number;
  code: string;
  name: string;
  description: string;
  price: number;
  commercialLine: string;
  category: string;
  images: string;
  isActive: boolean;
}