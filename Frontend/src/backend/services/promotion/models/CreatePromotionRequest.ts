export default interface CreatePromotionRequest {
  name: string;
  discountPercentage: number;
  validFrom: string;
  validTo: string;
}
