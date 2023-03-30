import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  canDeactivate(
    component: MemberEditComponent,      //which component are we tying to deactivate from
    ): boolean {
      if(component.editForm?.dirty)
      {
        return confirm("Are you sure you want to leave the page? Any unsaved changes will be lost");
      }
    return true;
  }
}
