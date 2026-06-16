import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from '@angular/common/http';
import { Observable, catchError, retry, throwError } from 'rxjs';

export default abstract class ApiRepository {
  protected fullEndpoint: string;

  protected get headers() {
    return {
      headers: new HttpHeaders({
        accept: 'application/json',
        Authorization: localStorage.getItem('token') ?? '',
        'ngrok-skip-browser-warning': 'true',
      }),
    };
  }

  // en el constructor recibo el url base de la api, el nombre del endpoint y el http client de angular
  constructor(
    protected readonly _apiOrigin: string,
    protected readonly _endpoint: string,
    protected readonly _http: HttpClient
  ) {
    this.fullEndpoint = `${this._apiOrigin}/${this._endpoint}`; // junta la url y el endpoint como veo en postman
  }

  protected get<T>(extraResource = '', query = ''): Observable<T> {
    query = query ? `?${query}` : '';
    extraResource = extraResource ? `/${extraResource}` : '';

    return this._http
      .get<T>(`${this.fullEndpoint}${extraResource}${query}`, this.headers)
      .pipe(retry(3), catchError(this.handleError));
  }

  protected post<T>(body: any, extraResource = ''): Observable<T> {
    extraResource = extraResource ? `/${extraResource}` : '';

    return this._http
      .post<T>(`${this.fullEndpoint}${extraResource}`, body, this.headers)
      .pipe(retry(3), catchError(this.handleError));
  }

  protected putById<T>(id: string, body: any = null, extraResource = ''): Observable<T> {
    extraResource = extraResource ? `/${extraResource}` : '';

    return this._http
      .put<T>(`${this.fullEndpoint}/${id}${extraResource}`, body, this.headers)
      .pipe(retry(3), catchError(this.handleError));
  }

  protected delete<T>(extraResource = '', query = ''): Observable<T> {
    query = query ? `?${query}` : '';
    extraResource = extraResource ? `/${extraResource}` : '';

    return this._http
      .delete<T>(`${this.fullEndpoint}${extraResource}${query}`, this.headers)
      .pipe(retry(3), catchError(this.handleError));
  }

  protected patch<T>(id: number, extraResource = '', body: any = null): Observable<T> {
    extraResource = extraResource ? `/${extraResource}` : '';

    return this._http
      .patch<T>(`${this.fullEndpoint}/${id}${extraResource}`, body, this.headers)
      .pipe(retry(3), catchError(this.handleError));
  }

  protected handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      console.error('An error occurred:', error.error.message);
    } else {
      console.error(
        `Backend returned code ${error.status}, body was: ${error.error}`
      );
    }
    return throwError(() => error.error?.message || 'Error en el servidor.');
  }
}
