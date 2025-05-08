import { Component } from '@angular/core';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-page-not-found',
  imports: [angularMaterialModulesUtil(), RouterModule],
  templateUrl: './page-not-found.component.html',
  styleUrl: './page-not-found.component.scss'
})
export class PageNotFoundComponent {

}
