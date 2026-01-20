import { NavLink } from 'react-router-dom';

interface SidebarItem {
  path: string;
  label: string;
  icon: string;
}

interface SidebarProps {
  items: SidebarItem[];
}

export function Sidebar({ items }: SidebarProps) {
  return (
    <aside className="ct-sidebar">
      <div className="ct-sidebar__container">
        <nav className="ct-sidebar__nav">
          {items.map((item) => (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                `ct-sidebar__item ${isActive ? 'ct-sidebar__item--active' : ''}`
              }
            >
              <span className="ct-sidebar__icon">{item.icon}</span>
              <span className="ct-sidebar__label">{item.label}</span>
            </NavLink>
          ))}
        </nav>
      </div>
    </aside>
  );
}
