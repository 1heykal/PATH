export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  birthDate: Date;
  role: string;
  organizationsCount: number;
  projectsCount: number;
  tasksCount: number;
}
