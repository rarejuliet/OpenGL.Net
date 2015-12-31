
// Copyright (C) 2013-2015 Luca Piccioni
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
// USA

using System;
using System.Collections.Generic;

namespace OpenGL
{
	/// <summary>
	/// Shader include library.
	/// </summary>
	class ShaderIncludeLibrary : GraphicsResource
	{
		#region Constructors

		/// <summary>
		/// Construct a ShaderIncludeLibrary.
		/// </summary>
		public ShaderIncludeLibrary()
		{
			// Load all include files defined by ShadersLibrary
			foreach (ShadersLibrary.Include shaderLibraryInclude in ShadersLibrary.Instance.Includes) {
				if (_IncludeFileSystem.ContainsKey(shaderLibraryInclude.Id))
					throw new InvalidOperationException(String.Format("include path '{0}' is duplicated", shaderLibraryInclude.Id));

				ShaderInclude shaderInclude = new ShaderInclude(shaderLibraryInclude.Id);

				shaderInclude.LoadSource(shaderLibraryInclude.Path);
				shaderInclude.IncRef();

				_IncludeFileSystem.Add(shaderInclude.IncludePath, shaderInclude);
			}
		}

		#endregion

		#region Shader Include Collection

		/// <summary>
		/// Determine whether an include source have a specific path.
		/// </summary>
		/// <param name="path">
		/// A <see cref="String"/> that specifies an include file path.
		/// </param>
		/// <returns>
		/// It returns a boolean value indicating whether <paramref name="path"/> specifies an include file.
		/// </returns>
		public bool IsPathDefined(string path)
		{
			return (_IncludeFileSystem.ContainsKey(path));
		}
		
		public ShaderInclude GetInclude(string path)
		{
			return (_IncludeFileSystem[path]);
		}
		
		/// <summary>
		/// Map between paths and include source strings.
		/// </summary>
		private readonly Dictionary<string, ShaderInclude> _IncludeFileSystem = new Dictionary<string, ShaderInclude>();
		
		#endregion
		
		#region GraphicsResource Overrides
		
		/// <summary>
		/// Shader include object class.
		/// </summary>
		internal static readonly Guid ShaderIncludeLibraryObjectClass = new Guid("2B702F5B-15D2-453A-9A42-4212C61AD650");

		/// <summary>
		/// Shader include object class.
		/// </summary>
		public override Guid ObjectClass { get { return (ShaderIncludeLibraryObjectClass); } }
		
		/// <summary>
		/// Determine whether this object requires a name bound to a context or not.
		/// </summary>
		/// <param name="ctx">
		/// A <see cref="GraphicsContext"/> used for creating this object name.
		/// </param>
		/// <returns>
		/// <para>
		/// This implementation returns always false.
		/// </para>
		/// </returns>
		protected override bool RequiresName(GraphicsContext ctx)
		{
			return (false);
		}
		
		/// <summary>
		/// Actually create this GraphicsResource resources.
		/// </summary>
		/// <param name="ctx">
		/// A <see cref="GraphicsContext"/> used for allocating resources.
		/// </param>
		protected override void CreateObject(GraphicsContext ctx)
		{
			foreach (ShaderInclude shaderInclude in _IncludeFileSystem.Values)
				shaderInclude.Create(ctx);
		}
		
		/// <summary>
		/// Delete this GraphicsResource.
		/// </summary>
		/// <param name="ctx">
		/// A <see cref="GraphicsContext"/> used for deleting this object. The IRenderResource shall belong to the object space to this
		/// <see cref="GraphicsContext"/>. The <see cref="GraphicsContext"/> shall be current to the calling thread.
		/// </param>
		/// <remarks>
		/// <para>
		/// After this method, the resource must have deallocated every graphic resource associated with it. Normally it should be possible
		/// to create again the resources by calling <see cref="Create"/>.
		/// </para>
		/// <para>
		/// This methods shall be the preferred way to deallocate graphic resources.
		/// </para>
		/// <para>
		/// After a successfull call to Create, <see cref="Exists"/> shall return true.
		/// </para>
		/// <para>
		/// The actual implementation deletes the name (<see cref="DeleteName"/>) only if the implementation requires a context related name
		/// (<see cref="RequiresName"/>). In the case derived classes requires more complex resource deletion pattern, this method could
		/// be overriden for that purpose, paying attention to call the base implementation.
		/// </para>
		/// </remarks>
		/// <seealso cref="Create"/>
		/// <exception cref="ArgumentNullException">
		/// Exception thrown if <paramref name="ctx"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Exception thrown if <paramref name="ctx"/> is not current to the calling thread.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Exception thrown if this object doesn't exists for <paramref name="ctx"/> (this is determined by calling <see cref="Exists"/>
		/// method), or this resource has a name and <paramref name="ctx"/> is not current to the calling thread.
		/// </exception>
		public override void Delete(GraphicsContext ctx)
		{
			// Dispose shader includes
			foreach (ShaderInclude shaderInclude in _IncludeFileSystem.Values)
				shaderInclude.DecRef();
			// Base implementation
			base.Delete(ctx);
		}
		
		#endregion
	}
}

