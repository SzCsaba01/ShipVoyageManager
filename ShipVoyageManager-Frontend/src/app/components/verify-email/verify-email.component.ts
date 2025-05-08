import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { takeUntil } from 'rxjs';
import { UserService } from '../../services/user.service';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-verify-email',
  imports: [CommonModule, RouterModule],
  templateUrl: './verify-email.component.html',
  styleUrl: './verify-email.component.scss'
})
export class VerifyEmailComponent extends SelfUnsubscriberBase implements OnInit {
  private token: string = '';
  constructor(
    private route: Router,
    private userService: UserService,
    private activatedRoute: ActivatedRoute
  ) {
    super();
  }

  ngOnInit(): void {
    this.token = this.activatedRoute.snapshot.paramMap.get("token") as string;

    this.userService.verifyEmailByRegistrationToken(this.token)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.route.navigate(['/login']);
        },
        error: () => {
          this.route.navigate(['/page_not_found']);
        }
      });
  }
}
