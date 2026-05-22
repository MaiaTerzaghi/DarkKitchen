export default interface UpdatePromotionRequest {
  name: string;
  discountPercentage: number;
  validFrom: string;
  validTo: string;
}
