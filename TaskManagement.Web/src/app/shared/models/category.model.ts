export interface Category {
  id: string;
  name: string;
  description: string | null;
  color: string;
  userId: string;
  createdAt: string;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string | null;
  color: string;
}

export type UpdateCategoryRequest = CreateCategoryRequest;
