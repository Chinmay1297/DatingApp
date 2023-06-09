import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { Observable, take } from 'rxjs';
import { Pagination } from 'src/app/_models/pagination';
import { UserParams } from '../../_models/userParams';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit{

  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams| undefined;
  user: User| undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];

  constructor(private memberService: MembersService, private accountService: AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user=>{
        if(user)
        {
          this.user = user;
          this.userParams = new UserParams(user);
        }
      }
    })
   }

  ngOnInit(): void {
    this.loadMembers();
    //this.members$ = this.memberService.getMembers();
  }

  loadMembers()
  {
    if(!this.userParams) return;
    this.memberService.getMembers(this.userParams).subscribe({
        //next: response => this.members = response
        next: response => {
          if(response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
    })
  }

  resetFilters(){
    if(this.user)
    {
      this.userParams = new UserParams(this.user);
      this.loadMembers();
    }
  }

  pageChanged(event : any)
  {
    if(this.userParams && this.userParams?.pageNumber!=event.page)
    {
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
  }

}
