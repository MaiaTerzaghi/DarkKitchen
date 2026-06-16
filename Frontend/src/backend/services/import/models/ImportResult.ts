export interface ImportError {
  index: number;
  code: string;
  reason: string;
}

export default interface ImportResult {
  importedCount: number;
  errors: ImportError[];
}
