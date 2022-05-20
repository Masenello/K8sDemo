import { Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ToastrService } from 'ngx-toastr';
import { UserDto } from 'src/app/_models/API_Messages/UserDto';
import { RoleToShortStringPipe } from 'src/app/_shared/Pipes/role-tostring.pipe';
import { RoleToLongStringPipe } from 'src/app/_shared/Pipes/role-tostring.pipe';
import { DialogService } from 'src/app/_shared/Services/dialog.service';
import { DialogIconType } from 'src/app/_shared/Services/dialog/dialog.model';
import { UsersService } from '../users.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {

  @ViewChild(MatSort) sort: MatSort;
  users: MatTableDataSource<UserDto>;
  searchFilter: string;

  constructor(private userService:UsersService, private dialogService:DialogService, private toastr:ToastrService) { }

  ngOnInit() {
    this.getUsers();
  }

  getUsers(): void {
    this.userService.getUsers().subscribe(users => {
      this.users = new MatTableDataSource<UserDto>(users);
      this.users.sort = this.sort;
      this.refreshView();
    });
  }

  refreshView(): void {
    this.users.filter = this.searchFilter?.trim().toLowerCase();
    if (this.users.paginator) {
      this.users.paginator.firstPage();
    }
  }

  resetFilters(): void {
    this.searchFilter = null;
    this.refreshView();
  }

  deleteUser(user: UserDto) {
    this.dialogService.confirm({
      title: 'Delete User',
      message: 'You are about to delete:\n<b>' + user.username + ' - ' + user.firstName + ' ' + user.lastName + '</b>\nConfirm?',
      icon: DialogIconType.Warning,
      isDisruptive: true,
      confirmText: 'Delete',
      confirmAction: () => {
        this.userService.deleteUser(user.username).subscribe(() => {
          this.toastr.success('User deleted!');
          this.getUsers();
        },error=>{
          console.log(error)
          this.toastr.error("User delete failed")
        });
      }
    })
  }

}
