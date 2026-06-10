export default interface OrderFilter {
  dateFrom?: string;
  dateTo?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}