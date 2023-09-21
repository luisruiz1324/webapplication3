import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { Observable, catchError, throwError } from "rxjs";


@Injectable()
export class ErrorInterceptor implements HttpInterceptor{

    constructor(private router: Router, private toastr: ToastrService){}


    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                this.toastr.error(error.error.message, error.status.toString());
                return throwError(() => new Error(error.message));
            })
        )
    }
    
}