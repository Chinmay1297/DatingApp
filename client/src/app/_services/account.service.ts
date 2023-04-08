import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment.development';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();                     //$ is a convention to say this is an observable
  
  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: any){
    return this.http.post<User>(this.baseUrl + 'account/login', model)      //<User> defines what typ of obj we'll get from http request
    .pipe(
      map((response: User)=> {
        const user = response;
        if(user)
        {
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model:any)
  {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user=>{
        if(user)
        {
          this.setCurrentUser(user);
        }

        return user; //since we are using map we have to return from here
      })
    )
  }

  setCurrentUser(user: User)
  {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles)? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user',JSON.stringify(user));
    this.currentUserSource.next(user);

    this.presenceService.createHubConnection(user);
  }

  logout()
  {
    //remove the item from localstorage
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token: string)
  {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
