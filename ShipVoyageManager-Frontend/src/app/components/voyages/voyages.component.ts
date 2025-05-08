import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { VoyageService } from '../../services/voyage.service';
import { Store } from '@ngrx/store';
import { MatTableDataSource } from '@angular/material/table';
import { IVoyage } from '../../models/voyage/voyage.model';
import { userRoles } from '../../constants/user-roles';
import { selectUserRole } from '../../state/authentication/auth.selector';
import { takeUntil } from 'rxjs';
import { ConfirmationModalComponent } from "../../shared-components/confirmation-modal/confirmation-modal.component";
import { EditVoyageModalComponent } from "../../shared-components/edit-voyage-modal/edit-voyage-modal.component";
import { AddVoyageModalComponent } from "../../shared-components/add-voyage-modal/add-voyage-modal.component";

@Component({
  selector: 'app-voyages',
  standalone: true,
  imports: [CommonModule, angularMaterialModulesUtil(), formModulesUtil(), ConfirmationModalComponent, EditVoyageModalComponent, AddVoyageModalComponent],
  templateUrl: './voyages.component.html',
  styleUrl: './voyages.component.scss'
})
export class VoyagesComponent extends SelfUnsubscriberBase implements OnInit {
  displayedColumns: string[] = []
  dataSource = new MatTableDataSource<any>([]);

  pageSize = 5;
  page = 0;
  totalVoyages = 0;
  pageSizeOptions = [5, 10, 25, 50];
  voyageToDelete: IVoyage = {} as IVoyage;

  isAdmin = false;

  isDeleteVoyageModalOpen = false;
  isAddVoyageModalOpen = false;
  isEditVoyageModalOpen = false;

  selectedVoyage: IVoyage = {} as IVoyage;

  constructor(
    private voyageService: VoyageService,
    private store: Store,
  ) {
    super();
  }

  ngOnInit(): void {
    this.store.select(selectUserRole).subscribe(role => {
      this.isAdmin = role === userRoles.admin;
      this.setDisplayedColumns();
    })
    this.loadData();
  }

  private setDisplayedColumns(): void {
    this.displayedColumns = ['departureDate', 'arrivalDate', 'shipName', 'departurePortName', 'arrivalPortName']
    if (this.isAdmin) {
      this.displayedColumns.push('delete');
    }
  }

  private loadData(): void {
    this.voyageService.getFilteredVoyagesPaginated(this.page, this.pageSize)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((response) => {
        this.dataSource.data = response.voyages;
        this.totalVoyages = response.totalCount;
      });
  }

  onPageChange(event: any): void {
    this.page = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  onDeleteClicked(voyage: IVoyage): void {
    this.voyageToDelete = voyage;
    this.isDeleteVoyageModalOpen = true;
  }

  onCloseDeleteModal(): void {
    this.isDeleteVoyageModalOpen = false;
  }

  onDeleteConfirmed(): void {
    this.voyageService.deleteVoyage(this.voyageToDelete.id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe(() => {
        this.isDeleteVoyageModalOpen = false;
        this.loadData();
      });
  }

  onAddVoyageClicked(): void {
    this.isAddVoyageModalOpen = true;
  }

  onAddVoyage(): void {
    this.isAddVoyageModalOpen = false;
    this.loadData();
  }

  onCloseAddVoyageModal(): void {
    this.isAddVoyageModalOpen = false;
  }

  onRowClicked(voyage: IVoyage): void {
    this.selectedVoyage = voyage;
    this.isEditVoyageModalOpen = true;
  }

  onCloseEditVoyageModal(): void {
    this.isEditVoyageModalOpen = false;
  }

  onEditVoyage(): void {
    this.isEditVoyageModalOpen = false;
    this.loadData();
  }
}
