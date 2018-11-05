import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-plugins',
  templateUrl: './plugins.component.html',
  styleUrls: ['./plugins.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PluginsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
