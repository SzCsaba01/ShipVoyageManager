import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { MatTableDataSource } from '@angular/material/table';
import { takeUntil } from 'rxjs';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { UserService } from '../../services/user.service';
import { FormControl, FormGroup } from '@angular/forms';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { IUser } from '../../models/user/user.model';
import { ConfirmationModalComponent } from "../../shared-components/confirmation-modal/confirmation-modal.component";

@Component({
  selector: 'app-manage-users',
  imports: [CommonModule, angularMaterialModulesUtil(), formModulesUtil(), ConfirmationModalComponent],
  templateUrl: './manage-users.component.html',
  styleUrl: './manage-users.component.scss'
})
export class ManageUsersComponent extends SelfUnsubscriberBase implements OnInit {
  displayedColumns: string[] = ['username', 'email', 'isEmailConfirmed', 'registrationDate', 'delete'];
  dataSource = new MatTableDataSource<any>([]);

  pageSize = 5;
  page = 0;
  totalUsers = 0;
  pageSizeOptions = [5, 10, 25, 50];
  isDeleteUserModalOpen = false;
  userToDelete: IUser = {} as IUser;

  filterFormGroup = {} as FormGroup;

  searchTerm = {} as FormControl;

  constructor(
    private userService: UserService,  
  ) {
    super();
  } 

  ngOnInit(): void {
    this.initializeForm();
    this.loadData();
  }

  private initializeForm() {
    this.searchTerm = new FormControl('');

    this.filterFormGroup = new FormGroup({
      searchTerm: this.searchTerm,
    });
  }

  private loadData() {
    this.userService
      .getFilteredUsersPaginated(this.page, this.pageSize, this.searchTerm.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((response) => {
        this.totalUsers = response.totalCount;
        this.dataSource.data = response.users; 
      });
  }

  onPageChange(event: any): void {
    this.page = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  applyFilter(): void {
    this.page = 0; 
    this.pageSize = 5;
    this.loadData();
  }

  onDeleteClicked(user: IUser): void {
    this.userToDelete = user;
    this.isDeleteUserModalOpen = true;
  }

  onCloseModal(): void {
    this.isDeleteUserModalOpen = false;
  }

  onDeleteConfirmed(): void {
    this.userService.deleteUser(this.userToDelete.username).pipe(takeUntil(this.ngUnsubscribe)).subscribe(() => {
      this.isDeleteUserModalOpen = false;
      this.loadData();
    });
  }
}
