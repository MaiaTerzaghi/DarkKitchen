export interface PromotionProduct {
  id: number;
  name: string;
}

export default interface PromotionResponse {
  id: number;
  name: string;
  discountPercentage: number;
  validFrom: string;
  validTo: string;
  productLine: string;
  products: PromotionProduct[];
}
