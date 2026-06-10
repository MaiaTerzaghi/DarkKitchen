export default interface OrderByDateFilter {
  dateFrom: string;
  dateTo: string;
  street?: string;
  status?: number;
  page?: number;
  pageSize?: number;
}
