import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-home.html',
  styleUrl: './admin-home.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminHome {}
