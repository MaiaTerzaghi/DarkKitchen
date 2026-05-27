export default interface ProductManageFilter {
  name?: string;
  description?: string;
  category?: string;
  commercialLine?: string;
  isActive?: boolean;
  priceMin?: number;
  priceMax?: number;
  page?: number;
  pageSize?: number;
}