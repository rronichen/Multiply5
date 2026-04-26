
import { Component, inject, OnDestroy } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Subscription, interval } from 'rxjs';
import { MultiplyService, RateLimitResponse } from './multiply.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnDestroy {
  
  private readonly multiplyService: any = inject(MultiplyService);
  private countdownSubscription?: Subscription;
 
  inputValue = '';
  result: boolean | null = null;
  errorMessage = '';
  isRateLimited = false;
  countdown = 0;
 
  check(): void {
    this.errorMessage = '';
    this.result = null;
 
    this.multiplyService.check(this.inputValue).subscribe({
      next: (response:  any) => (this.result = response.isMultiplyOfFive),
      error: (err: HttpErrorResponse) => {
        if (err.status === 429) {
          this.startCountdown((err.error as RateLimitResponse).retryAfterSeconds);
        } else {
          this.errorMessage = err.error ?? 'An unexpected error occurred.';
        }
      },
    });
  }
 
  private startCountdown(seconds: number): void {
    this.isRateLimited = true;
    this.countdown = seconds;
 
    this.countdownSubscription = interval(1000).subscribe(() => {
      this.countdown--;
      if (this.countdown <= 0) {
        this.stopCountdown();
      }
    });
  }
 
  private stopCountdown(): void {
    this.isRateLimited = false;
    this.countdownSubscription?.unsubscribe();
  }
 
  ngOnDestroy(): void {
    this.countdownSubscription?.unsubscribe();
  }
}