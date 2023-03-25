import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model:any = {};
  currentUser$: Observable<User | null> = of(null);

  constructor(private accountService: AccountService){}

  ngOnInit(): void {
   this.currentUser$ = this.accountService.currentUser$;
  }

  // getCurrentUser()
  // {
  //   this.accountService.currentUser$.subscribe({
  //     next: user => this.loggedIn = !!user,             // !!makes the expression boolean same as if user = true else false
  //     error: err=> console.log(err)
  //   })
  // }

  login()
  {
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: err => {
        console.log(err)
      },
      complete: ()=>{
        //turn off loader
      }
    });
  }

  logout()
  {
    this.accountService.logout();
  }
}
