import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ImportApiRepositoryService } from '../../repositories/import-api-repository.service';
import ImporterInfo from './models/ImporterInfo';
import ImportResult from './models/ImportResult';

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  constructor(private readonly _repository: ImportApiRepositoryService) {}

  public getImporters(): Observable<ImporterInfo[]> {
    return this._repository.getImporters();
  }

  public importProducts(
    importerName: string,
    content: string,
    fileName: string
  ): Observable<ImportResult> {
    return this._repository.importProducts(importerName, content, fileName);
  }
}
