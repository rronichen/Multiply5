import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CheckMultiplyResponse {
  isMultiplyOfFive: boolean;
}

export interface RateLimitResponse {
  retryAfterSeconds: number;
  availableAt: string;
}

@Injectable({ providedIn: 'root' })
export class MultiplyService {
  private readonly apiUrl = 'https://localhost:5000/api/multiply/check';
  private readonly http = inject(HttpClient);

  check(value: string): Observable<CheckMultiplyResponse> {
    return this.http.post<CheckMultiplyResponse>(this.apiUrl, { value });
  }
}