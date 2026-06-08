import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import ApiRepository from './api-repository';
import environments from '../../environments/environment';
import ImporterInfo from '../services/import/models/ImporterInfo';
import ImportResult from '../services/import/models/ImportResult';

@Injectable({
  providedIn: 'root',
})
export class ImportApiRepositoryService extends ApiRepository {
  constructor(http: HttpClient) {
    super(environments.darkKitchenApi, 'imports', http);
  }

  public getImporters(): Observable<ImporterInfo[]> {
    return this.get<ImporterInfo[]>('importers');
  }

  public importProducts(
    importerName: string,
    content: string,
    fileName: string
  ): Observable<ImportResult> {
    return this.post<ImportResult>({ importerName, content, fileName });
  }
}
