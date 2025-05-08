import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
  private messageSubject = new Subject<{ message: string, type: 'success' | 'error' }>();

  message$ = this.messageSubject.asObservable();

  showMessage(message: string, type: 'success' | 'error') {
    this.messageSubject.next({ message, type });
  }
}
