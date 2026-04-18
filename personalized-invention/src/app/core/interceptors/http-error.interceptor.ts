import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

// New Angular uses a function, not a class
export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'An unexpected error occurred.';
      if (error.status === 404) message = 'Resource not found.';
      if (error.status === 400) message = error.error?.message || 'Bad request.';
      if (error.status === 0)   message = 'Cannot reach the server. Is the API running?';
      console.error(`[PI Error] ${error.status}: ${message}`);
      return throwError(() => new Error(message));
    })
  );
};