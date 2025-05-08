import { Component, OnInit } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { PortService } from '../../services/port.service';
import { MatTableDataSource } from '@angular/material/table';
import { IPort } from '../../models/port/port.model';
import { FormGroup, FormControl } from '@angular/forms';
import { takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { ConfirmationModalComponent } from '../../shared-components/confirmation-modal/confirmation-modal.component';
import { AddPortModalComponent } from "../../shared-components/add-port-modal/add-port-modal.component";
import { EditPortModalComponent } from "../../shared-components/edit-port-modal/edit-port-modal.component";
import { Store } from '@ngrx/store';
import { userRoles } from '../../constants/user-roles';
import { selectUserRole } from '../../state/authentication/auth.selector';

@Component({
  selector: 'app-ports',
  standalone: true,
  imports: [CommonModule, angularMaterialModulesUtil(), formModulesUtil(), ConfirmationModalComponent, AddPortModalComponent, EditPortModalComponent],
  templateUrl: './ports.component.html',
  styleUrl: './ports.component.scss'
})
export class PortsComponent extends SelfUnsubscriberBase implements OnInit {
  displayedColumns: string[] = ['name', 'countryName'];
  dataSource = new MatTableDataSource<any>([]);

  pageSize = 5;
  page = 0;
  totalPorts = 0;
  pageSizeOptions = [5, 10, 25, 50];
  portToDelete: IPort = {} as IPort;

  isDeletePortModalOpen = false;
  isAddPortModalOpen = false;
  isEditPortModalOpen = false;

  isAdmin = false;

  selectedPort: IPort = {} as IPort;

  filterFormGroup = {} as FormGroup;

  searchTerm = {} as FormControl;

  constructor(
    private portService: PortService,
    private store: Store
  ) {
    super();
  }

  ngOnInit(): void {
    this.store.select(selectUserRole).subscribe(role => {
      this.isAdmin = role === userRoles.admin;
      this.setDisplayedColumns();
    });
    this.initializeForm();
    this.loadData();
  }

  private setDisplayedColumns(): void {
    this.displayedColumns = ['name', 'countryName'];
    if (this.isAdmin) {
      this.displayedColumns.push('delete');
    }
  }

  private initializeForm(): void {
    this.searchTerm = new FormControl('');

    this.filterFormGroup = new FormGroup({
      searchTerm: this.searchTerm,
    });
  }

  private loadData(): void {
    this.portService
      .getFilteredPortsPaginated(this.page, this.pageSize, this.searchTerm.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((response) => {
        this.totalPorts = response.totalCount;
        this.dataSource.data = response.ports; 
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

  onDeleteClicked(port: IPort): void {
    this.isDeletePortModalOpen = true;
    this.portToDelete = port;
  }

  onCloseDeleteModal(): void {
    this.isDeletePortModalOpen = false;
  }

  onDeleteConfirmed(): void {
    this.portService.deletePort(this.portToDelete.id).subscribe(() => {
      this.isDeletePortModalOpen = false;
      this.loadData();
    });
  }

  onAddPortClicked(): void {
    this.isAddPortModalOpen = true;
  }

  onAddPort(): void {
    this.isAddPortModalOpen = false;
    this.loadData();
  }

  onCloseAddPortModal(): void {
    this.isAddPortModalOpen = false;
  }

  onRowClicked(port: IPort): void {
    this.isEditPortModalOpen = true;
    this.selectedPort = port;
  }

  onCloseEditPortModal(): void {
    this.isEditPortModalOpen = false;
  }

  onEditPort(): void {
    this.isEditPortModalOpen = false;
    this.loadData();
  }

}
