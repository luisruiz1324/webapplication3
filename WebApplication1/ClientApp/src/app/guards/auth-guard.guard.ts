import { Injectable, inject} from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
  constructor(private authService: AuthService, private router: Router, private toastService: ToastrService){

  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot):  boolean {
    if(localStorage.getItem('token')){
       const { role } = route.data;
       if(role && !role.includes(localStorage.getItem('role'))){
        this.toastService.error('Not an admin. Cant access this page. Try registering or logging in as an admin')
        this.router.navigateByUrl('home')
        return false;
       }
      return true;
    }
    this.router.navigateByUrl('login').then(() => window.location.reload())
    return false;
  }
  
}

export const IsAuthGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot):boolean => {
  return inject(AuthGuard).canActivate(route, state);
}

