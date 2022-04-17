import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../Func_Login/account.service';
import { MenuColor, MenuEntry, MenuIconType, MenuLocation, MenuSection } from './menu-entry.model';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {

  constructor(private router: Router,
    private accountService:AccountService) {

  }

  public navigate(pageName: string, state?: any): void {
    console.log(`Navigating to ${pageName}`)
    this.router.navigate([pageName], { state: state });
  }

  public navigateByQuery(pageName: string, queryParams: any): void {
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
    this.router.navigate([pageName], { queryParams: queryParams }));
  }

  
  public get menuColors(): typeof MenuColor {
    return MenuColor;
  }

  public get menuLocations(): typeof MenuLocation {
    return MenuLocation;
  }

  public get menuIconTypes(): typeof MenuIconType {
    return MenuIconType;
  }


  public findMenu(menus: MenuEntry[], id: string): MenuEntry {
    for (let menu of menus) {
      if (menu.pageName == id) {
        return menu;
      }
      if (menu.subMenus) {
        var foundMenu = this.findMenu(menu.subMenus, id);
        if (foundMenu) {
          return foundMenu;
        }
      }
    }
    return null;
  }

  public sortMenusBySection(menus: MenuEntry[]): Map<MenuSection, MenuEntry[]> {
    var result = new Map();

    if (menus) {
      menus.forEach(menu => {
        if (!result.has(menu.section)) {
          result.set(menu.section, []);
        }
        result.get(menu.section).push(menu);
      })
    }

    return result;
  }

  public getMenuRoute(menus: MenuEntry[], id: string): MenuEntry[] {
    for (let menu of menus) {
      var foundRoute = this.getMenuRouteRecursive(menu, id, []);
      if (foundRoute) {
        return foundRoute;
      }
    }
    return null;
  }

  private getMenuRouteRecursive(menu: MenuEntry, id: string, route: MenuEntry[]): MenuEntry[] {
    if (menu.pageName == id) {
      return route;
    }
    if (menu.subMenus) {
      for (let submenu of menu.subMenus) {
        var foundRoute = this.getMenuRouteRecursive(submenu, id, route.concat(menu));
        if (foundRoute) {
          return foundRoute;
        }
      }
    }
    return foundRoute;
  }

  public isMenuAccessible(menu: MenuEntry, location: MenuLocation): boolean {

    return menu && (!menu.locations || menu.locations.includes(location)) && this.accountService.hasRole(menu.roles);
  }

  public isSectionAccessible(section: [MenuSection, MenuEntry[]], location: MenuLocation): boolean {
    if (!section) { return false; }
    for (let menu of section[1]) {
      if (this.isMenuAccessible(menu, location)) {
        return true;
      }
    }
    return false;
  }

  public accessibleSubmenus(menu: MenuEntry, location: MenuLocation): MenuEntry[] {
    return menu?.subMenus?.filter(x => (!x.locations || x.locations.includes(location)) && this.accountService.hasRole(x.roles));
  }

  public getUserMenus(menus: MenuEntry[]): MenuEntry[] {
    var result: MenuEntry[] = [];

    if (menus) {
      menus.forEach(menu => {
        if (!menu.subMenus) {
          if (menu.locations?.includes(MenuLocation.UserMenu) && this.accountService.hasRole(menu.roles)) {
            result = result.concat(menu);
          }
        }
        else {
          result = result.concat(this.getUserMenus(menu.subMenus));
        }
      })
    }

    return result;
  }
}
