import { HttpInterceptorFn } from '@angular/common/http';
import { tap } from 'rxjs';

export const durationInterceptor: HttpInterceptorFn = (req, next) => {
  const started = performance.now();

  return next(req).pipe(
    tap({
      finalize() {
        logDuration(req.url, started);
      },
    })
  );
};

function logDuration(url: string, started: number): void {
  const duration = (performance.now() - started).toFixed(2);
  console.log(`[API] ${url} – ${duration}ms`);
}