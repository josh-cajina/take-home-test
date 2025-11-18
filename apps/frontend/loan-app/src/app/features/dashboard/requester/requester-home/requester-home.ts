import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-requester-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './requester-home.html',
  styleUrl: './requester-home.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RequesterHome {}
