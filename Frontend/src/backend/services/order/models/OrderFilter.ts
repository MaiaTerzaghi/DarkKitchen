export default interface OrderFilter {
  dateFrom?: string;
  dateTo?: string;
  street?: string;
  status?: string | number;
}