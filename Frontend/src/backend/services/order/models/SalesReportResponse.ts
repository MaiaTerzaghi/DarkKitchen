export interface ClientSales {
  clientId: number;
  clientName: string;
  total: number;
}

export interface SalesReportMonth {
  year: number;
  month: number;
  clients: ClientSales[];
  monthlyTotal: number;
}

export default interface SalesReportResponse {
  months: SalesReportMonth[];
  generalTotal: number;
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
