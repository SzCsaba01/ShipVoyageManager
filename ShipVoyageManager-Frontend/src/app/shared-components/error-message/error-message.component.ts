import { Component, OnInit } from '@angular/core';
import { ErrorHandlerService } from '../../services/error-handler.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-message',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './error-message.component.html',
  styleUrl: './error-message.component.scss'
})
export class ErrorMessageComponent implements OnInit {
  message = '';
  type = {} as 'success' | 'error' as string;
  private subscription = {} as Subscription;
  isVisible = false; 

  constructor(private errorHandlerService: ErrorHandlerService) { }

  ngOnInit() {
    this.subscription = this.errorHandlerService.message$.subscribe(({ message, type }) => {
      if (message) {
        this.message = message; 
        this.type = type;
        this.isVisible = true; 
        setTimeout(() => this.closeMessage(), 5000);
      }
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  closeMessage() {
    this.message = '';
    this.isVisible = false;
  }
}
