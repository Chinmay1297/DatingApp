import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment.development';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Route, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private rotuter: Router) { }

  createHubConnection(user: User)
  {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: ()=> user.token
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start().catch(error=>{
        console.log(error);
      });

      this.hubConnection.on('UserIsOnline', username=> {
        this.toastr.success(username + ' is now online');
      });

      this.hubConnection.on('UserIsOffline', username=>{
        this.toastr.warning(username + ' has disconnected');
      });

      this.hubConnection.on("GetOnlineUsers", usernames => {
        this.onlineUsersSource.next(usernames);
      });

      this.hubConnection.on("NewMessageReceived", ({username, knownAs})=> {
        this.toastr.info(knownAs+ 'has sent you a new message! Click to see it.')
        .onTap.pipe(take(1))
        .subscribe({
          next: ()=>{
            this.rotuter.navigateByUrl('/members/'+ username + '?tab=Messages');
          }
        })
      })
  }

  stopHubConnection()
  {
    this.hubConnection?.stop().catch(error=> console.log(error));
  }
}
