import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-analyst-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './analyst-home.html',
  styleUrl: './analyst-home.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AnalystHome {}
